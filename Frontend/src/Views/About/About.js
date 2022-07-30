import './About.css';
import React from 'react';
import config from '../../config';

class About extends React.Component {
    render(){
        return(
            <div>
                <div className='minecraft title'>
                    About {config.name}
                </div>
                <div className='h2 row'>
                    <div className='col-10'>         
                        Cubelist is a place where you can find great Minecraft servers. Ranking is only based on amount of players online.
                        No buying votes or premium accounts. Everything is free and open source. Have fun!
                    </div>
                    <div className='col-2'>
                        <img src='logo.png' alt={config.name} width="150px" className='img-fluid float-end'/>
                    </div>
                    <br />
                    <a className='githubLink' href='https://github.com/Nasus20202/MCserverList' target={'_blank'} rel={'noreferrer'}>Check my spaghetti code</a>
                </div>
            </div>
        )
    }
}

export default About;