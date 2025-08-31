import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { appConfig } from './app/app.config.js';

import { routes } from './app/app.routes.js';
import { AppComponent } from './app/app.js';

bootstrapApplication(AppComponent, {
  providers:[
    provideRouter(routes)
  ]
})
  .catch((err) => console.error(err));
