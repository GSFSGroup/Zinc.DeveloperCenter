import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { BackendService } from '~/core/backend-service/backend.service';
import { AdrSummary } from '~/models/adr-summary.interface';
import { Page } from '~/models/page.interface';
import { RepositoryListComponent } from '~/models/repo.interface';

@Injectable({providedIn: 'root'})
export class GitHubRepoService {
    public constructor(private backend: BackendService){}

    public listRepos(): Observable<Page<RepositoryListComponent>> {
        return this.backend.query<Page<RepositoryListComponent>>('repos');
    }

    public listApps(): Observable<Page<RepositoryListComponent>> {
        return this.backend.query<Page<RepositoryListComponent>>('applications');
    }

    public searchApps(searchQuery: string): Observable<Page<AdrSummary>> {
        return this.backend.query<Page<AdrSummary>>(`architecture-decision-records/search?q=${encodeURIComponent(searchQuery)}`);
    }
}
