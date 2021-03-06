import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { of } from 'rxjs';

import { LoadingOverlayService } from '~/core/loading-module/services/loading-overlay/loading-overlay.service';
import { AdrSummary } from '~/models/adr-summary.interface';
import { Page } from '~/models/page.interface';
import { GitHubAdrService } from '~/shared/services/github-adr.service';

import { AdrSummaryComponent } from './adr-summary-list.component';

describe('AdrSummaryComponent', () => {
    let component: AdrSummaryComponent;
    let fixture: ComponentFixture<AdrSummaryComponent>;
    let adrService: jasmine.SpyObj<GitHubAdrService>;
    let loadingOverlayService: jasmine.SpyObj<LoadingOverlayService>;
    const adrPage: Page<AdrSummary> = {
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
        adrService = jasmine.createSpyObj<GitHubAdrService>('adrService', {
            listAdrs: of(adrPage)
        });
        loadingOverlayService = jasmine.createSpyObj<LoadingOverlayService>('loadingOverlayService', ['show', 'hide']);

        await TestBed.configureTestingModule({
            declarations: [AdrSummaryComponent],
            imports: [HttpClientTestingModule],
            providers: [
                { provide: GitHubAdrService, useValue: adrService },
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
