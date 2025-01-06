import * as React from 'react';
import { Route, Routes } from 'react-router-dom';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { Actions } from './components/Actions';

export const routes = <Routes>
    <Route path='/' element={<Layout><Home /></Layout>} />
    <Route path='/Actions' element={<Layout><Actions/></Layout>} />
</Routes>;
