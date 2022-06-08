import { ErrorHandler, Injectable } from '@angular/core';
import { SharedServices } from '@gsfsgroup/kr-shell-services';

import { Logger } from '~/core/logger/logger';

@Injectable({
    providedIn: 'root'
})
export class ErrorHandlerService implements ErrorHandler {
    public constructor(private sharedServices: SharedServices, private logger: Logger) {}

    public handleError(error: any) {
        this.logger.error(`An unexpected error occurred in Typescript: ${error.message || ''}`, error);
        console.error(error);
        this.sharedServices.notification.showError('Uh oh, something went wrong. Please try again.');
    }
}
