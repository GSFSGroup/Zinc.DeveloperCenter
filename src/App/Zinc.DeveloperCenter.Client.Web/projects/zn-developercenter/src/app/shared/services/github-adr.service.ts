import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { BackendService } from '~/core/backend-service/backend.service';
import { AdrContent } from '~/models/adr-content.interface';
import { AdrSummary } from '~/models/adr-summary.interface';
import { Page } from '~/models/page.interface';

@Injectable({providedIn: 'root'})
export class GitHubAdrService {
    public constructor(private backend: BackendService){}

    public listAdrs(applicationName: string, sortedOn: string, sortAsc: boolean): Observable<Page<AdrSummary>> {
        return this.backend.query<Page<AdrSummary>>(`adrs/${applicationName}/details/sorted-on/${sortedOn}/sort-asc/${sortAsc}`);
    }

    public listAppAdrs(applicationName: string): Observable<Page<AdrSummary>> {
        return this.backend.query<Page<AdrSummary>>(`architecture-decision-records/${applicationName}`);
    }

    public downloadAdrContent(applicationName: string, filePath: string): Observable<AdrContent> {
        return this.backend.query<AdrContent>(`architecture-decision-records/download/${applicationName}?path=${encodeURIComponent(filePath)}`);
    }

    public updateDates(applicationName: string, adrTitle: string): Observable<Date> {
        return this.backend.query<Date>(`adrs/update-dates/${applicationName}/${adrTitle}`);
    }
}
