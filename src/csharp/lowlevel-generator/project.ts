/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

import { Import } from '#csharp/code-dom/import';
import { Project as codeDomProject } from '#csharp/code-dom/project';
import { JsonSerializerClass } from '#csharp/lowlevel-generator/support/json-serializer';
import { State } from './generator';
import { ModelsNamespace } from './model/namespace';
import { ApiClass } from './operation/api-class';
import { ServiceNamespace } from './operation/namespace';
import { SupportNamespace } from './support/namespace';

export class Project extends codeDomProject {
  public storagePipeline: boolean = false;
  public jsonSerialization: boolean = true;
  public xmlSerialization: boolean = false;
  public defaultPipeline: boolean = true;
  public emitSignals: boolean = true;

  constructor(protected state: State) {
    super();
    state.project = this;
    // add project namespace

  }

  public async init(): Promise<this> {
    await super.init();

    const service = this.state.service;
    this.storagePipeline = await service.GetValue('use-storage-pipeline') || false;
    if (this.storagePipeline) {
      this.emitSignals = false;
      this.jsonSerialization = false;
      this.xmlSerialization = true;
      this.defaultPipeline = false;
    }

    this.addNamespace(this.serviceNamespace = new ServiceNamespace(this.state));

    // add support namespace
    this.addNamespace(this.supportNamespace = new SupportNamespace(this.serviceNamespace, this.state));

    // add model classes
    this.addNamespace(this.modelsNamespace = new ModelsNamespace(this.serviceNamespace, this.state.model.schemas, this.state.path('components', 'schemas')));

    // create API class
    new ApiClass(this.serviceNamespace, this.state);

    if (this.jsonSerialization) {
      // create serialization support
      new JsonSerializerClass(this.supportNamespace, this.state);
    }
    this.modelsNamespace.addUsing(new Import(this.supportNamespace.fullName));

    // abort now if we have any errors.
    this.state.checkpoint();

    return this;
  }

  public serviceNamespace!: ServiceNamespace;
  public modelsNamespace!: ModelsNamespace;
  public supportNamespace!: SupportNamespace;
}
