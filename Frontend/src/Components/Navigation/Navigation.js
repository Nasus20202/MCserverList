import React from 'react';
import './Navigation.css'
import { UrlContext, NameContext } from '../../App';
import Container from 'react-bootstrap/Container';
import Nav from 'react-bootstrap/Nav';
import Navbar from 'react-bootstrap/Navbar';
import { Link } from "react-router-dom";

class Navigation extends React.Component {
    render(){
        return (
            <div>
                <UrlContext.Consumer>
                {(url) => (
                <NameContext.Consumer>
                {(name) => (
                    <Navbar  fixed="top" collapseOnSelect expand="sm" bg="dark" variant="dark" className='topNavbar'>
                    <Container>
                    <Navbar.Brand><Link to="/" className='minecraft brand'><img src="logo192.png" width="35" height="35" className="d-inline-block align-top" alt=""/>{' '}{name}</Link></Navbar.Brand>
                      <Navbar.Toggle aria-controls="responsive-navbar-nav" />
                      <Navbar.Collapse id="responsive-navbar-nav">
                        <div className="me-auto">
                        </div>
                        <Nav>
                          <Nav><Link className='links' to="/new">Submit your server!</Link></Nav>
                          <Nav><Link className='links' to="/ADEAAA2D-551F-4B8A-BFF5-B0D91BE4B8D1">About {name}</Link></Nav>
                        </Nav>
                      </Navbar.Collapse>
                    </Container>
                  </Navbar>
                )}
                </NameContext.Consumer>
                )}
                </UrlContext.Consumer>
            </div>
        )
    }
}

export default Navigation;