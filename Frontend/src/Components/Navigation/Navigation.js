import React from 'react';
import './Navigation.css'
import config from '../../config.json';
import Container from 'react-bootstrap/Container';
import Nav from 'react-bootstrap/Nav';
import Navbar from 'react-bootstrap/Navbar';
import { Link } from "react-router-dom";
import { BsFillSunFill, BsMoonFill } from 'react-icons/bs';

class Navigation extends React.Component {
    render(){
        return (
            <div>
              <Navbar  fixed="top" collapseOnSelect expand="sm" bg="dark" variant="dark" className='topNavbar'>
              <Container>
              <Navbar.Brand><Link to="/" className='minecraft brand'><img src="logo192.png" width="35" height="35" className="d-inline-block align-top" alt=""/>{' '}{config.name}</Link></Navbar.Brand>
                <Navbar.Toggle aria-controls="responsive-navbar-nav" />
                <Navbar.Collapse id="responsive-navbar-nav">
                  <div className="me-auto">
                  </div>
                  <Nav>
                    <Nav><Link className='links' to="/new">Submit your server!</Link></Nav>
                    <Nav><Link className='links' to="/about">About {config.name}</Link></Nav>
                    <Nav className='links' style={{marginTop:'4px'}} onClick={this.props.toggleTheme}>{this.props.theme === 'dark' ? <BsFillSunFill /> : <BsMoonFill />}</Nav>
                  </Nav>
                </Navbar.Collapse>
              </Container>
            </Navbar>
            </div>
        )
    }
}

export default Navigation;