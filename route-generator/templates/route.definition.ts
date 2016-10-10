import {
    Request,
} from '@angular/http';

import {
    json,
    MockServerRouter,
} from '../backend';

import {
    ConfigurationService,
} from '../../../common';

export class <--CLASS--> {
    static endpoints = (router: MockServerRouter) => {
        let config = new ConfigurationService();

<--ROUTES-->
    };
}
