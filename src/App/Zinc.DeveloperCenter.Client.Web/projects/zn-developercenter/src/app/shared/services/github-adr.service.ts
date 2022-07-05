import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { BackendService } from '~/core/backend-service/backend.service';
import { AdrSummary } from '~/models/adr-summary.interface';
import { Page } from '~/models/page.interface';

@Injectable({providedIn: 'root'})
export class GitHubAdrService {
    public constructor(private backend: BackendService){}

    public listAdrs(repoDotName: string): Observable<Page<AdrSummary>> {
        return this.backend.query<Page<AdrSummary>>(`adrs/${repoDotName}/details`);
    }
}
