import React from "react";
import { createRoot } from 'react-dom/client';
import { BrowserRouter } from "react-router-dom";
import { routes } from "./routes";
import '../src/css/site.css';
import '../src/css/vendor.css';
//import 'bootstrap';

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href')!;
const root = createRoot(document.getElementById('root')!);

root.render(
  <React.StrictMode>
        <BrowserRouter basename={baseUrl}>
          {routes}
        </BrowserRouter>
  </React.StrictMode>,
);