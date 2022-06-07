import { TestBed } from '@angular/core/testing';
import { SharedServices } from '@gsfsgroup/kr-shell-services';

import { Logger } from './logger';


describe('Logger', () => {
    let logger: Logger;
    const service = 'zn-developercenter';
    const context = { ctx: 'extra property' };
    const message = 'logged message';
    const sharedServices: SharedServices = new SharedServices();

    beforeEach(() => {
        TestBed.configureTestingModule({
            declarations: [],
            imports: [],
            providers: [Logger, { provide: SharedServices, useValue: sharedServices }],
            schemas: []
        });
    });

    beforeEach(() => {
        logger = TestBed.inject(Logger);
    });

    it('should be created', () => {
        expect(logger).toBeTruthy();
    });

    it('should send errors', () => {
        spyOn(sharedServices.logger, 'error');

        logger.error(message, context);

        expect(sharedServices.logger.error).toHaveBeenCalledWith(service, message, context);
    });

    it('should send warnings', () => {
        spyOn(sharedServices.logger, 'warn');

        logger.warn(message, context);

        expect(sharedServices.logger.warn).toHaveBeenCalledWith(service, message, context);
    });

    it('should send info', () => {
        spyOn(sharedServices.logger, 'info');

        logger.info(message, context);

        expect(sharedServices.logger.info).toHaveBeenCalledWith(service, message, context);
    });

    it('should send debug', () => {
        spyOn(sharedServices.logger, 'debug');

        logger.debug(message, context);

        expect(sharedServices.logger.debug).toHaveBeenCalledWith(service, message, context);
    });
});
