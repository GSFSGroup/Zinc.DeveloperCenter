import { HttpClientTestingModule } from '@angular/common/http/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { SharedServices } from '@gsfsgroup/kr-shell-services';

import { HomeComponent } from './home.component';

describe('HomeComponent', () => {
    let component: HomeComponent;
    let fixture: ComponentFixture<HomeComponent>;
    const sharedServices: SharedServices = new SharedServices();

    beforeEach(
        waitForAsync(() => {
            spyOn(sharedServices.breadCrumbs, 'addCrumb');
            TestBed.configureTestingModule({
                imports: [HttpClientTestingModule],
                providers: [{ provide: SharedServices, useValue: sharedServices }],
                declarations: [HomeComponent],
                schemas: [NO_ERRORS_SCHEMA]
            }).compileComponents();
        })
    );

    beforeEach(() => {
        fixture = TestBed.createComponent(HomeComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    it('should contain a Read More button that is clickable', () => {
        const compiled = fixture.debugElement.nativeElement;
        fixture.detectChanges();
        const h1 = compiled.querySelector('h1');
        const title = h1.textContent;

        expect(title).toContain('Welcome to Zinc.DeveloperCenter Micro-App');
    });
});
