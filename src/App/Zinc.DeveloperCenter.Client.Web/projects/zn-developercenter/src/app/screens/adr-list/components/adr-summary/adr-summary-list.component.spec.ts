import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';

import { LoadingOverlayService } from '~/core/loading-module/services/loading-overlay/loading-overlay.service';
import { Page } from '~/models/page.interface';
import { Repo } from '~/models/repo.interface';
import { GitHubRepoService } from '~/shared/services/github-repo.service';

import { AdrSummaryComponent } from './adr-summary-list.component';

describe('AdrSummaryComponent', () => {
    let component: AdrSummaryComponent;
    let fixture: ComponentFixture<AdrSummaryComponent>;
    let repoService: jasmine.SpyObj<GitHubRepoService>;
    let loadingOverlayService: jasmine.SpyObj<LoadingOverlayService>;
    const repoPage: Page<Repo> = {
        hasNextPage: false,
        hasPreviousPage: false,
        isFirstPage: false,
        isLastPage: false,
        items: [],
        page: 0,
        pageSize: 0,
        totalItems: 0,
        totalPages: 0
    };

    beforeEach(async () => {
        repoService = jasmine.createSpyObj<GitHubRepoService>('repoService', {
            listRepos: of(repoPage)
        });
        loadingOverlayService = jasmine.createSpyObj<LoadingOverlayService>('loadingOverlayService', ['show', 'hide']);

        await TestBed.configureTestingModule({
            declarations: [AdrSummaryComponent],
            imports: [HttpClientTestingModule],
            providers: [
                { provide: GitHubRepoService, useValue: repoService },
                { provide: LoadingOverlayService, useValue: loadingOverlayService }
            ]
        })
            .compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(AdrSummaryComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
