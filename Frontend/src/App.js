import './App.css';
import React from 'react';
import config from './config.json';
import Cookies from 'js-cookie';
import 'bootstrap/dist/css/bootstrap.min.css';
import Navigation from './Components/Navigation/Navigation';
import { Outlet } from "react-router-dom";
import Container from 'react-bootstrap/esm/Container';

class App extends React.Component {
  constructor(props){
    super(props);
    this.state = {
      url: config.url,
      name: config.name,
      mode: Cookies.get('mode') || 'dark'
    }
    this.handleThemeChange = this.handleThemeChange.bind(this);
    if(this.state.mode === "dark")
      document.getElementsByTagName('body')[0].className = 'dark';
  }

  handleThemeChange(event){
    var body = document.getElementsByTagName('body')[0];
    if(this.state.mode === 'light') {
      this.setState({
        mode: 'dark'
      });
      Cookies.set('mode', 'dark');
      body.className = 'dark';
    } else{
      this.setState({
        mode: 'light'
      });
      Cookies.set('mode', 'light');
      body.classList.remove('dark');
    }
  }

  render(){ 
    document.title = this.state.name;
    return (
      <div>
        <Navigation toggleTheme={this.handleThemeChange} theme={this.state.mode}/>
        <Container className="app">
          <Outlet />
        </Container>
      </div>
  );
  }
}

export default App;
