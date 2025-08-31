import { bootstrapApplication } from '@angular/platform-browser';
import { App } from './app/app.js';
import { config } from './app/app.config.server.js';

const bootstrap = () => bootstrapApplication(App, config);

export default bootstrap;
