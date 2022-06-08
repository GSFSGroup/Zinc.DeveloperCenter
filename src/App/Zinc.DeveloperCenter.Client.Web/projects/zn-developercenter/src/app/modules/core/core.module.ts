// The CoreModule takes on the role of the root AppModule ,
// but is not the module which gets bootstrapped by Angular at run-time.
// The CoreModule should contain singleton services (which is usually the case),
// universal components and other features where there’s only once instance per application.
// To prevent re-importing the core module elsewhere, you should also add a guard for it in the core module’ constructor.

import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { ErrorHandler, NgModule, Optional, SkipSelf } from '@angular/core';
import { SharedServices } from '@gsfsgroup/kr-shell-services';

import { ErrorHandlerService } from '~/core/error-handler/error-handler.service';
import { Logger } from '~/core/logger/logger';
import { MaterialModule } from '~/core/material-module/material.module';
import { throwIfAlreadyLoaded } from '~/core/module-import-guard';

@NgModule({
    declarations: [],
    imports: [HttpClientModule, CommonModule, MaterialModule],
    exports: [CommonModule, HttpClientModule, MaterialModule],
    providers: [
        SharedServices,
        { provide: ErrorHandler, useClass: ErrorHandlerService },
        Logger
    ]
})
export class CoreModule {
    public constructor(@Optional() @SkipSelf() parentModule: CoreModule) {
        throwIfAlreadyLoaded(parentModule, 'CoreModule');
    }
}
