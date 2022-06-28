import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { BackendService } from '~/core/backend-service/backend.service';
import { Page } from '~/models/page.interface';
import { AdrSummary } from '~/models/adr-summary.interface';

@Injectable({providedIn: 'root'})
export class GitHubAdrService {
    public constructor(private backend: BackendService){}

    public listTemplateAdrs(): Observable<Page<AdrSummary>> {
        return this.backend.query<Page<AdrSummary>>('adrs');
    }

    public listAdrs(repoDotName: string): Observable<Page<AdrSummary>> {
        return this.backend.query<Page<AdrSummary>>('adrs/{repoDotName}');
    }
}
