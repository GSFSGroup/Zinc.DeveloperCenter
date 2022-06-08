import { Injectable } from '@angular/core';

import { BackendService } from '~/core/backend-service/backend.service';

@Injectable({
    providedIn: 'root'
})
export class HomeService {
    public constructor(private backendService: BackendService) {}
}
