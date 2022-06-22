import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MaterialModule } from '~/core/material-module/material.module';

import { LoadingOverlayComponent } from './loading-overlay.component';

describe('LoadingOverlayComponent', () => {
    let component: LoadingOverlayComponent;
    let fixture: ComponentFixture<LoadingOverlayComponent>;

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            declarations: [LoadingOverlayComponent],
            imports: [MaterialModule]
        }).compileComponents();
    });

    beforeEach(() => {
        fixture = TestBed.createComponent(LoadingOverlayComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });
});
