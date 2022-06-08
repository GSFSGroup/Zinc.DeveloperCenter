import { Injectable } from '@angular/core';
import { SharedServices } from '@gsfsgroup/kr-shell-services';

@Injectable({
    providedIn: 'root'
})
export class Logger {
    private serviceName = 'zn-developercenter';

    public constructor(private sharedServices: SharedServices) { }

    /**
     * Writes an error level log entry.
     *
     * @param message The message for the event.
     * @param context An object with any additional attributes attatched to the message.
     */
    public error(message: string, context?: any): void {
        this.sharedServices.logger.error(this.serviceName, message, context);
    }

    /**
     * Writes a warning level log entry.
     *
     * @param message The message for the event.
     * @param context An object with any additional attributes attatched to the message.
     */
    public warn(message: string, context?: any): void {
        this.sharedServices.logger.warn(this.serviceName, message, context);
    }

    /**
     * Writes an info level log entry.
     *
     * @param message The message for the event.
     * @param context An object with any additional attributes attatched to the message.
     */
    public info(message: string, context?: any): void {
        this.sharedServices.logger.info(this.serviceName, message, context);
    }

    /**
     * Writes a debug level log entry.
     *
     * @param message The message for the event.
     * @param context An object with any additional attributes attatched to the message.
     */
    public debug(message: string, context?: any): void {
        this.sharedServices.logger.debug(this.serviceName, message, context);
    }
}
