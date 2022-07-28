import React from 'react';
import './ServerThumbnail.css'
import { ThemeContext } from '../../../App';
import {Link} from 'react-router-dom';

class ServerThumbnail extends React.Component{
    static contextType = ThemeContext;
    constructor(props){
        super(props);
        this.server = this.props.server;
    }
    render(){
        return(
            <Link to={"/" + this.server.serverId}>
            <div className={this.context === 'dark' ? 'serverCard row' : 'serverCard serverCard-light row'}>
                <div className='col-1' style={{minWidth:'150px'}}>
                    <img src={this.server.image} alt={this.server.name} className='serverCard-image'/>
                </div>
                <div className='col'>
                    <div className='serverCard-name'>{this.server.name}</div>
                    <div className='serverCard-motd'>{this.server.motd}</div>
                    <div className='serverCard-players'>{this.server.players}/{this.server.maxPlayers}</div></div>
            </div>
            </Link>
        )
    }
}

export default ServerThumbnail;