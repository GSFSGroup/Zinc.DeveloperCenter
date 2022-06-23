import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

import { LoadingOverlayService } from '~/core/loading-module/services/loading-overlay/loading-overlay.service';
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
    public fetchedRepos = false;

    private destroyed$ = new Subject<void>();

    public constructor(
        private repoService: GitHubRepoService,
        private loadingService: LoadingOverlayService
    ) { }

    public ngOnInit(): void {
        this.getGSFSGitHubReposInit();
    }

    public ngOnDestroy(): void {
        this.destroyed$.next();
        this.destroyed$.complete();
    }

    public getGSFSGitHubReposInit(): void {
        this.loadingService.show('Loading');
        this.repoService.listRepos()
            .pipe(takeUntil(this.destroyed$))
            .subscribe(repos => {
                this.repos = repos;
                this.loadingService.hide();
                this.fetchedRepos = true;
            });
    }
}
