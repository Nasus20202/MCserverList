import "./App.css";
import React from "react";
import { apiURL, appName } from "./config";
import "bootstrap/dist/css/bootstrap.min.css";
import Navigation from "./Components/Navigation/Navigation";
import { Outlet } from "react-router-dom";
import Container from "react-bootstrap/esm/Container";

const ThemeContext = React.createContext("dark");

class App extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      mode: localStorage.getItem("mode") || "dark",
    };
    this.handleThemeChange = this.handleThemeChange.bind(this);
    if (this.state.mode === "dark")
      document.getElementsByTagName("body")[0].className = "dark";
  }

  handleThemeChange(event) {
    var body = document.getElementsByTagName("body")[0];
    if (this.state.mode === "light") {
      this.setState({
        mode: "dark",
      });
      localStorage.setItem("mode", "dark");
      body.className = "dark";
    } else {
      this.setState({
        mode: "light",
      });
      localStorage.setItem("mode", "light");
      body.classList.remove("dark");
    }
  }

  render() {
    document.title = appName;
    return (
      <div>
        <ThemeContext.Provider value={this.state.mode}>
          <Navigation
            toggleTheme={this.handleThemeChange}
            theme={this.state.mode}
          />
          <Container className="app">
            <Outlet />
          </Container>
        </ThemeContext.Provider>
      </div>
    );
  }
}

export default App;
export { ThemeContext };
