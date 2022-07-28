import './App.css';
import React from 'react';
import config from './config.json';
import Cookies from 'js-cookie';
import 'bootstrap/dist/css/bootstrap.min.css';
import Navigation from './Components/Navigation/Navigation';
import { Outlet } from "react-router-dom";

const UrlContext = React.createContext(null);
const NameContext = React.createContext(null);

class App extends React.Component {
  constructor(props){
    super(props);
    this.state = {
      url: config.url,
      name: config.name,
      mode: Cookies.get('mode') || 'light'
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
    const button = <div onClick={this.handleThemeChange}>{this.state.mode === "light" ? "Dark mode" : "Light mode"}</div> 
    document.title = this.state.name;
    return (
    <UrlContext.Provider value={this.state.url}>
      <NameContext.Provider value={this.state.name}>
      <Navigation />
      {this.state.name}<br/>
      {this.state.url}<br />
      {button}
      <Outlet />
      </NameContext.Provider>
    </UrlContext.Provider>
  );
  }
}

export default App;
export { UrlContext, NameContext };
