import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { Page } from '~/models/page.interface';
import { ADRSummary, Repo } from '~/models/repo.interface';
import { GitHubRepoService } from '~/shared/services/github-repo.service';

@Component({
    selector: 'app-adr-list',
    templateUrl: './adr-list.component.html',
    styleUrls: ['./adr-list.component.scss']
})
export class AdrListComponent implements OnInit, OnDestroy {
    public repos!: Page<Repo>;
    public templateADRs!: ADRSummary[];
    public fetchedRepos: boolean = false;

    private destroyed$ = new Subject<void>();

    public constructor(
        private repoService: GitHubRepoService,
        private router: Router
    ) { }

    public ngOnInit(): void {
        this.repoService.listRepos()
            .pipe(takeUntil(this.destroyed$))
            .subscribe(repos => this.repos = repos);
        this.fetchedRepos = true;
    }

    public ngOnDestroy(): void {
        this.destroyed$.next();
        this.destroyed$.complete();
    }
}
