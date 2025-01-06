import './css/site.css';
import 'bootstrap';
import * as React from 'react';
import { createRoot } from 'react-dom/client';
import { BrowserRouter } from 'react-router-dom';
import * as RoutesModule from './routes';
const routes = RoutesModule.routes;

function renderApp() {
    // This code starts up the React app when it runs in a browser. It sets up the routing
    // configuration and injects the app into a DOM element.
    const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href')!;
    const root = createRoot(document.getElementById('react-app')!);
    root.render(<BrowserRouter {...{children:routes }} basename={ baseUrl } />);
}

renderApp();