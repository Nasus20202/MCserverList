import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import App from './App';
import reportWebVitals from './reportWebVitals';
import ServerList from './Views/ServerList/ServerList';
import ServerPage from './Views/ServerPage/ServerPage';
import NewServer from './Views/NewServer/NewServer';
import About from './Views/About/About';
import { BrowserRouter, Routes, Route } from "react-router-dom";

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <BrowserRouter>
    <Routes>
      <Route path="/" element={<App />}>
        <Route index element={<ServerList />} />
        <Route path=":serverId" element={<ServerPage />} />
        <Route path="/new" element={<NewServer />} />
        <Route path="/about" element={<About />} />
      </Route>

    </Routes>
  </BrowserRouter>
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
