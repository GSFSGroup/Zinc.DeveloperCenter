import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { BackendService } from '~/core/backend-service/backend.service';
import { Page } from '~/models/page.interface';
import { RepositoryListComponent } from '~/models/repo.interface';

@Injectable({providedIn: 'root'})
export class GitHubRepoService {
    public constructor(private backend: BackendService){}

    public listRepos(): Observable<Page<RepositoryListComponent>> {
        return this.backend.query<Page<RepositoryListComponent>>('repos');
    }
}
