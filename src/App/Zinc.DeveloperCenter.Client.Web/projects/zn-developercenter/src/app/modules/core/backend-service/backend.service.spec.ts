import { HttpClientTestingModule, HttpTestingController, TestRequest } from '@angular/common/http/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { TestBed, waitForAsync } from '@angular/core/testing';
import { IHttpHeader, SharedServices } from '@gsfsgroup/kr-shell-services';

import { BackendService } from './backend.service';

// tslint:disable-next-line:no-big-function
describe('BackendService', () => {
    let service: BackendService;
    let httpBackend: HttpTestingController;
    const testEndPoint = 'testEndPoint';
    const sharedServices: SharedServices = new SharedServices();
    const baseUrl = 'baseurl';
    const tenantId = 'testing';
    const authHeader: IHttpHeader = {
        name: 'auth',
        value: 'auth'
    };
    const correlationIdHeader: IHttpHeader = {
        name: 'correlationId',
        value: 'correlationId'
    };

    beforeEach(
        waitForAsync(() => {
            spyOn(sharedServices.registry, 'baseUrlFor').withArgs('zn-developercenter').and.returnValue(baseUrl);
            spyOn(sharedServices.configuration, 'getTenantId').and.returnValue(tenantId);
            spyOn(sharedServices.headers, 'getAuthenticationHeader').and.returnValue(authHeader);
            spyOn(sharedServices.headers, 'getCorrelationIdHeader').and.returnValue(correlationIdHeader);

            TestBed.configureTestingModule({
                declarations: [],
                imports: [HttpClientTestingModule],
                providers: [BackendService, { provide: SharedServices, useValue: sharedServices }],
                schemas: [NO_ERRORS_SCHEMA]
            });
        })
    );

    beforeEach(() => {
        service = TestBed.inject(BackendService);
        httpBackend = TestBed.inject(HttpTestingController);
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    it('build the correct url for the request', () => {
        service.get<string>(testEndPoint).subscribe();
        const expectedUrl = `${baseUrl}/api/v1/${tenantId}/testEndPoint`;
        const call: TestRequest = httpBackend.expectOne(expectedUrl);

        expect(call.request.method).toEqual('GET');
    });

    it('build the correct method for put requests', () => {
        service
            .put<any>(testEndPoint, { somedata: 'data' })
            .subscribe();
        const expectedUrl = `${baseUrl}/api/v1/${tenantId}/testEndPoint`;
        const call: TestRequest = httpBackend.expectOne(expectedUrl);

        expect(call.request.method).toEqual('PUT');
    });

    it('build the correct method for post requests', () => {
        service
            .post<any>(testEndPoint, { somedata: 'data' })
            .subscribe();
        const expectedUrl = `${baseUrl}/api/v1/${tenantId}/testEndPoint`;
        const call: TestRequest = httpBackend.expectOne(expectedUrl);

        expect(call.request.method).toEqual('POST');
    });

    it('should build the correct url and method for postQuery', () => {
        service.postQuery<any, any>('testEndpoint', {key1: 'value1', key2: 'value2'}).subscribe();
        const expectedUrl = `${baseUrl}/ux/v1/${tenantId}/testEndpoint`;
        const call: TestRequest = httpBackend.expectOne(expectedUrl);

        expect(call.request.method).toEqual('POST');
    });

    it('should build correct url and method for query', () => {
        service.query<any>('testEndpoint').subscribe();
        const expectedUrl = `${baseUrl}/ux/v1/${tenantId}/testEndpoint`;
        const call: TestRequest = httpBackend.expectOne(expectedUrl);

        expect(call.request.method).toEqual('GET');
    });

    it('build the correct method for delete requests', () => {
        service.delete<any>(testEndPoint).subscribe();
        const expectedUrl = `${baseUrl}/api/v1/${tenantId}/testEndPoint`;
        const call: TestRequest = httpBackend.expectOne(expectedUrl);

        expect(call.request.method).toEqual('DELETE');
    });

    it('should add correlation id header for the request', () => {
        service.get<string>(testEndPoint).subscribe();
        const expectedUrl = `${baseUrl}/api/v1/${tenantId}/testEndPoint`;
        const call: TestRequest = httpBackend.expectOne(expectedUrl);

        expect(call.request.headers.has(correlationIdHeader.name)).toBeTruthy();
        expect(call.request.headers.get(correlationIdHeader.name)).toBe(correlationIdHeader.value);
    });

    it('should add the authentication header for the request', () => {
        service.get<string>(testEndPoint).subscribe();
        const expectedUrl = `${baseUrl}/api/v1/${tenantId}/testEndPoint`;
        const call: TestRequest = httpBackend.expectOne(expectedUrl);

        expect(call.request.headers.has(authHeader.name)).toBeTruthy();
        expect(call.request.headers.get(authHeader.name)).toBe(authHeader.value);
    });

    it('should add the content type header for the request', () => {
        service.get<string>(testEndPoint).subscribe();
        const expectedUrl = `${baseUrl}/api/v1/${tenantId}/testEndPoint`;
        const call: TestRequest = httpBackend.expectOne(expectedUrl);

        expect(call.request.headers.has('Content-Type')).toBeTruthy();
        expect(call.request.headers.get('Content-Type')).toBe('application/json');
    });

    it('should add the Accept type header for the request', () => {
        service.get<string>(testEndPoint).subscribe();
        const expectedUrl = `${baseUrl}/api/v1/${tenantId}/testEndPoint`;
        const call: TestRequest = httpBackend.expectOne(expectedUrl);

        expect(call.request.headers.has('Accept')).toBeTruthy();
        expect(call.request.headers.get('Accept')).toBe('application/json');
    });

    it('should add the payload as a query string to the request', () => {
        service
            .get<string>(testEndPoint, { a: 1 })
            .subscribe();
        const expectedUrl = `${baseUrl}/api/v1/${tenantId}/testEndPoint?a=1`;
        const call: TestRequest = httpBackend.expectOne(expectedUrl);

        expect(call.request.params).not.toBeNull();
        expect(call.request.urlWithParams.endsWith('?a=1')).toBeTruthy();
    });

    it('should encode simple objects to query string', () => {
        const payload = { a: 'value1', b: 'value2' };
        const expectedQueryString = 'a=value1&b=value2';

        const encodedQueryString = service.encodeQueryString(payload);

        expect(encodedQueryString).toBe(expectedQueryString);
    });

    it('should encode arrays to query string', () => {
        const payload = { a: ['value1', 'value2'] };
        const expectedQueryString = 'a%5B0%5D=value1&a%5B1%5D=value2'; // a[0]=value1&a[1]=value2

        const encodedQueryString = service.encodeQueryString(payload);

        expect(encodedQueryString).toBe(expectedQueryString);
    });

    it('should encode complex objects to query string', () => {
        const payload = { a: 'value1', b: { c: 'value2' } };
        const expectedQueryString = 'a=value1&b.c=value2';

        const encodedQueryString = service.encodeQueryString(payload);

        expect(encodedQueryString).toBe(expectedQueryString);
    });

    it('should encode null objects to blank query string', () => {
        const payload = null;
        const expectedQueryString = '';

        const encodedQueryString = service.encodeQueryString(payload);

        expect(encodedQueryString).toBe(expectedQueryString);
    });

    it('should not encode empty properties to query string', () => {
        const payload = { a: 'value1', b: null, c: [] };
        const expectedQueryString = 'a=value1';

        const encodedQueryString = service.encodeQueryString(payload);

        expect(encodedQueryString).toBe(expectedQueryString);
    });

    describe('ETagged requests', () => {
        const expectedEndPoint = `${baseUrl}/api/v1/${tenantId}/${testEndPoint}`;
        const payload = { a: 'value1', b: null, c: [] };
        const eTag = 'abc123';

        it('should have If-Match on put', () => {
            service.put<any>(testEndPoint, payload, eTag).subscribe();
            const call: TestRequest = httpBackend.expectOne(expectedEndPoint);

            expect(call.request.headers.get('If-Match')).toBe(eTag);
        });

        it('should have If-Match on delete', () => {
            service.delete<any>(testEndPoint, eTag).subscribe();
            const call: TestRequest = httpBackend.expectOne(expectedEndPoint);

            expect(call.request.headers.get('If-Match')).toBe(eTag);
        });

        it('should have If-Match on post', () => {
            service.post<any>(testEndPoint, payload, eTag).subscribe();
            const call: TestRequest = httpBackend.expectOne(expectedEndPoint);

            expect(call.request.headers.get('If-Match')).toBe(eTag);
        });

        it('should have If-None-Match on get', () => {
            service.get<any>(testEndPoint, null, eTag).subscribe();
            const call: TestRequest = httpBackend.expectOne(expectedEndPoint);

            expect(call.request.headers.get('If-None-Match')).toBe(eTag);
        });
    });

    describe('Non-ETagged requests', () => {
        const expectedEndPoint = `${baseUrl}/api/v1/${tenantId}/${testEndPoint}`;
        const payload = { a: 'value1', b: null, c: [] };

        it('should not have If-Match on put', () => {
            service.put<any>(testEndPoint, payload).subscribe();
            const call: TestRequest = httpBackend.expectOne(expectedEndPoint);

            expect(call.request.headers.has('If-Match')).toBeFalsy();
        });

        it('should not have If-Match on post', () => {
            service.post<any>(testEndPoint, payload).subscribe();
            const call: TestRequest = httpBackend.expectOne(expectedEndPoint);

            expect(call.request.headers.has('If-Match')).toBeFalsy();
        });

        it('should not have If-Match on delete', () => {
            service.delete<any>(testEndPoint).subscribe();
            const call: TestRequest = httpBackend.expectOne(expectedEndPoint);

            expect(call.request.headers.has('If-Match')).toBeFalsy();
        });

        it('should not have If-None-Match on get', () => {
            service.get<any>(testEndPoint).subscribe();
            const call: TestRequest = httpBackend.expectOne(expectedEndPoint);

            expect(call.request.headers.has('If-None-Match')).toBeFalsy();
        });
    });
});
