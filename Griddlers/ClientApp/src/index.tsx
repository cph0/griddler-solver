import React from "react";
import ReactDOM from "react-dom";
import { BrowserRouter } from "react-router-dom";
import { routes } from "./routes";
import '../src/css/site.css';
import '../src/css/vendor.css';
//import 'bootstrap';

const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href')!;

ReactDOM.render(
  <React.StrictMode>
        <BrowserRouter children={routes} basename={baseUrl} />
  </React.StrictMode>,
  document.getElementById('root')
);