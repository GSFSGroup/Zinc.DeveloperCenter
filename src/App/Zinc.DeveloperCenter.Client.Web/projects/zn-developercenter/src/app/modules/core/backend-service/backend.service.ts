import { Location } from '@angular/common';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { SharedServices } from '@gsfsgroup/kr-shell-services';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Injectable({
    providedIn: 'root'
})
export class BackendService {
    private apiBase: string;
    private uxBase: string;

    public constructor(private http: HttpClient, private sharedServices: SharedServices) {
        const tenantId = this.sharedServices.configuration.getTenantId();
        this.apiBase = `api/v1/${tenantId}/`;
        this.uxBase = `ux/v1/${tenantId}/`;
    }

    // HTTP GET ETag fetching comes in two flavors. Be sure to follow your individual micro-app's architectural design style.
    // Style A: Only singletons return an ETag in the header for concurrency. This prevents a leaky abstraction and is the prefered method.
    // Style B: All domain models that have concurrency contain the ETag. This means singleton and collection responses provide ETags in the result rather than the header. The singleton will likely contain the ETag, but it'll be redundant.
    // When requesting a collection, ETag should not be provided. Singleton requests will return cached data should the server-side implement the 304 response.
    // If the ETag is not known, it can be null.
    public get<T>(url: string, payload: any = null, eTag?: string): Observable<T> {
        if (payload) {
            url = `${url}?${this.encodeQueryString(payload)}`;
        }
        const headers = this.getHeaders(eTag ? { 'If-None-Match': eTag } : undefined);
        return this.http.get<T>(this.buildFullUrl(url), { headers: headers });
    }

    /** Make HEAD request to the given url and return headers. */
    public head(url: string): Observable<HttpHeaders> {
        const headers = this.getHeaders();
        return this.http
            .head(this.buildFullUrl(url, this.apiBase), { headers: headers, observe: 'response' })
            .pipe(map(response => response.headers));
    }

    // UXQueries do not support eTags.  Use an Api GET endpoint.
    public query<T>(url: string, payload: any = null): Observable<T> {
        if (payload) {
            url = `${url}?${this.encodeQueryString(payload)}`;
        }
        const headers = this.getHeaders();
        return this.http.get<T>(this.buildFullUrl(url, this.uxBase), { headers: headers });
    }

    public postQuery<TRequest, TResponse>(url: string, query: TRequest): Observable<TResponse> {
        const fullUrl = this.buildFullUrl(url, this.uxBase);
        const headers = this.getHeaders();
        return this.http.post<TResponse>(fullUrl, query,  { headers: headers });
    }

    // HTTP POST using ETags will be rare, but should be an option should the occaision arise. Performing searches, or other non-data-changing requests should ignore the If-Match.
    public post<T>(url: string, request: any, eTag?: string): Observable<T> {
        const headers = this.getHeaders(eTag ? { 'If-Match': eTag } : undefined);
        const requestUrl = this.buildFullUrl(url);
        return this.http.post<T>(requestUrl, request, { headers: headers });
    }
    // HTTP PUT using ETags will be regular, particularly for aggregate roots. Providing an ETag for If-Match allows the server to check concurrency. If creating a new record, the ETag should be null.
    public put<T>(url: string, request: any, eTag?: string): Observable<T> {
        const headers = this.getHeaders(eTag ? { 'If-Match': eTag } : undefined);
        const requestUrl = this.buildFullUrl(url);
        return this.http.put<T>(requestUrl, request, { headers: headers });
    }

    // HTTP DELETE using ETags will be regular, particularly for aggregate roots. Providing an ETag for If-Match allows the server to check concurrency. If No ETag is provided, a concurrency error should be returned (412).
    public delete<T>(url: string, eTag?: string): Observable<T> {
        const headers = this.getHeaders(eTag ? { 'If-Match': eTag } : undefined);
        const requestUrl = this.buildFullUrl(url);
        return this.http.delete<T>(requestUrl, { headers: headers });
    }

    public encodeQueryString(obj: any): string {
        return this.encodeQueryStringRecursion(obj, undefined);
    }

    // eslint-disable-next-line @typescript-eslint/ban-types
    private getHeaders(otherHeaders?: object): HttpHeaders {
        otherHeaders = otherHeaders || {};
        const headers = Object.assign(
            {
                'Content-Type': 'application/json',
                Accept: 'application/json'
            },
            otherHeaders
        );


        [
            this.sharedServices.headers.getAuthenticationHeader(),
            this.sharedServices.headers.getCorrelationIdHeader()
        ].forEach(h => {
            const name = h.name as keyof typeof headers;
            headers[name] = h.value;
        });

        return new HttpHeaders(headers);
    }

    private buildFullUrl(partial: string, basePath?: string): string {
        const rootUrl = this.sharedServices.registry.baseUrlFor('zn-developercenter');
        const result = [rootUrl, basePath || this.apiBase, partial].reduce((base, part) => Location.joinWithSlash(base, part));
        return result;
    }

    // eslint-disable-next-line @typescript-eslint/ban-types
    private encodeQueryStringRecursion(obj: object, prefix?: string): any {
        const str = [];

        for (const property in obj) {
            if (obj.hasOwnProperty(property)) {
                // eslint-disable-line @typescript-eslint/no-unsafe-call
                const accessor = Array.isArray(obj) ? `[${property}]` : `.${property}`;
                const propertyKey = prefix ? `${prefix}${accessor}` : property;
                const propertyName = property as keyof typeof obj;
                const propertyValue = obj[propertyName] as any;

                // don't bother printing values that are null or empty
                if (
                    propertyValue === null ||
                    propertyValue === undefined ||
                    (Array.isArray(propertyValue) && !propertyValue.length)
                ) {
                    continue;
                }

                str.push(
                    propertyValue !== null && typeof propertyValue === 'object'
                        ? this.encodeQueryStringRecursion(propertyValue, propertyKey)
                        : encodeURIComponent(propertyKey) + '=' + encodeURIComponent(propertyValue)
                );
            }
        }

        return str.join('&');
    }
}
