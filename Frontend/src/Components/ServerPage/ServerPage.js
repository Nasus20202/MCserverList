import './ServerPage.css';
import React from 'react';
import { useParams } from 'react-router-dom';

function ServerPage() {
    const { serverId } = useParams();
    return (
        <div className='h3'>
            Server {serverId}
        </div>
    );
}

export default ServerPage;