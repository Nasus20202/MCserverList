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
        let place = !isNaN(this.props.index) ? '#' + (this.props.index+1).toString() : '';
        return(
            <Link to={"/" + this.server.serverId}>
            <div className={this.context === 'dark' ? 'serverCard' : 'serverCard serverCard-light'}>
                <div className='serverCard-image-container' style={{minWidth:'150px'}}>
                    <img src={this.server.image} alt={this.server.name} className='serverCard-image'/>
                </div>
                <div className='serverCard-content'>
                    <div className='serverCard-name'><span className='serverCard-place'>{place} </span>{this.server.name}</div>
                    <div className='serverCard-motd'>{this.server.motd}</div>
                    <div className='serverCard-players'>{this.server.players}/{this.server.maxPlayers}</div></div>
                </div>
            </Link>
        )
    }
}

export default ServerThumbnail;