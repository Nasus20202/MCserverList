import './ServerDetails.css';
import React from 'react';
import config from '../../../config.json';
import Tags from '../../../Components/Tags/Tags';
import Motd from '../../../Components/Motd/Motd';

class ServerDetails extends React.Component {

    constructor(props){
        super(props);
        this.serverId = this.props.serverId;
        this.state = {
            server: {tags: []},
        }
    }

    async getServer(){
        const data = await fetch(
            `${config.url}servers/${this.serverId}`);
        const json = await data.json();
        if(data.status !== 200)
            json["name"] = "Server not found";
        return json;
    }

    async componentDidMount(){
        this.setState({
            server: await this.getServer(),
        });
    }
    render() {
        let tags;
        let motd;
        if(this.state.server.tags === undefined)
            tags = <div>Loading...</div>;
        else
            tags = <Tags tags={this.state.server.tags}/>
        if(this.state.server.motd === undefined)
            motd = <div>Loading...</div>
        else
            motd = <div  className='detailsMotd'><Motd motd={this.state.server.motd}/></div>
        return (
            <div>
                <div className='title minecraft'>
                    <img src={this.state.server.image} alt={this.state.server.serverId} className='logo serverDetailLogo'/>
                    {this.state.server.name}
                </div>
                <div className='about-details minecraft'>
                    <span className='players'>Players: {this.state.server.players}/{this.state.server.maxPlayers} </span>
                    <span className='version'>Version: {this.state.server.version}</span>
                </div>
                {motd}
                {this.state.server.about}<br/>
                {tags}
            </div>
        );
    }
}

export default ServerDetails;