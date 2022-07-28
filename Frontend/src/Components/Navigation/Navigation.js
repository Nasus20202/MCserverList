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
                    <Navbar collapseOnSelect expand="sm" bg="dark" variant="dark">
                    <Container>
                    <Link to="/"><Navbar.Brand>{name}</Navbar.Brand></Link>
                      <Navbar.Toggle aria-controls="responsive-navbar-nav" />
                      <Navbar.Collapse id="responsive-navbar-nav">
                        <div className="me-auto">
                        </div>
                        <Nav>
                          <Nav.Link><Link className='links' to="/new">Submit your server!</Link></Nav.Link>
                          <Nav.Link><Link className='links' to="/ADEAAA2D-551F-4B8A-BFF5-B0D91BE4B8D1">About {name}</Link></Nav.Link>
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