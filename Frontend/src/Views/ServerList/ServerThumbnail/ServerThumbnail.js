import React from 'react';
import './ServerThumbnail.css'
import { ThemeContext } from '../../../App';

class ServerThumbnail extends React.Component{
    static contextType = ThemeContext;
    constructor(props){
        super(props);
        this.server = this.props.server;
    }
    render(){
        return(
            <div className={this.context === 'dark' ? 'serverCard' : 'serverCard serverCard-light'}>
                {this.server.name}
            </div>
        )
    }
}

export default ServerThumbnail;