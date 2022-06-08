import { fakeAsync, TestBed, tick, waitForAsync } from '@angular/core/testing';
import { Router } from '@angular/router';
import { RouterTestingModule } from '@angular/router/testing';
import { SharedServices } from '@gsfsgroup/kr-shell-services';

import { AppComponent } from './app.component';
import { EmptyComponent } from './modules/empty/empty.component';

describe('AppComponent', () => {
    const sharedServices: SharedServices = new SharedServices();
    const testRoutes = [{
        path: 'level0',
        data: {
            breadcrumb: {
                title: 'Level 0',
                description: 'Level 0'
            }
        },
        children: [
            {
                path: 'level1',
                data: {
                    breadcrumb: {
                        title: 'Level 1',
                        description: 'Level 1'
                    }
                },
                component: EmptyComponent
            }
        ]
    }];
    let router: Router;

    beforeEach(
        waitForAsync(() => {
            spyOn(sharedServices.events, 'dispatchAppInitialized');

            TestBed.configureTestingModule({
                imports: [RouterTestingModule.withRoutes(testRoutes)],
                providers: [{ provide: SharedServices, useValue: sharedServices }],
                declarations: [AppComponent]
            }).compileComponents();

            router = TestBed.inject(Router);
        })
    );

    it('should create the app', () => {
        const fixture = TestBed.createComponent(AppComponent);
        const app = fixture.debugElement.componentInstance;

        expect(app).toBeTruthy();
    });

    it('should dispatchAppInitialized', () => {
        const fixture = TestBed.createComponent(AppComponent);
        fixture.componentInstance.ngOnInit();

        expect(sharedServices.events.dispatchAppInitialized).toHaveBeenCalled();
        const args = (sharedServices.events.dispatchAppInitialized as any).calls.argsFor(0)[0];

        expect(args.title).toBeTruthy();
        expect(args.icon).toBeTruthy();
        expect(args.icon.type).toBeTruthy();
        expect(args.icon.value).toBeTruthy();
    });

    it('should add breadcrumbs based on route data', fakeAsync(() => {
        const addCrumb = spyOn(sharedServices.breadCrumbs, 'addCrumb');
        const fixture = TestBed.createComponent(AppComponent);
        fixture.componentInstance.ngOnInit();
        router.navigate(['level0', 'level1']);
        tick();

        expect(addCrumb).toHaveBeenCalledTimes(2);
        expect(addCrumb).toHaveBeenCalledWith(jasmine.objectContaining({title: 'Level 0', level: 0}));
        expect(addCrumb).toHaveBeenCalledWith(jasmine.objectContaining({title: 'Level 1', level: 1}));
    }));
});
