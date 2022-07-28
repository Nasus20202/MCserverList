import React from 'react';
import './ServerThumbnail.css'

class ServerThumbnail extends React.Component{
    constructor(props){
        super(props);
        this.server = this.props.server;
    }

    render(){
        return(
            <div style={{marginBottom:'500px'}}>
                {this.server.name}
                <img src={this.server.image} alt={this.server.name}></img>
                {this.server.players}/{this.server.maxPlayers}
            </div>
        )
    }
}

export default ServerThumbnail;