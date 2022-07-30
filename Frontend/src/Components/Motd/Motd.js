import React from 'react';
import './Motd.css'
import colors from './colors';

class Motd extends React.Component
{
    constructor(props){
        super(props);
        this.motd = [];
        let bold = false;
        let prevColor = '';
        this.props.motd.split('\u00a7').forEach((element, index)=> {
            let colorCode = element.slice(0,1);
            const text = element.slice(1).replace('\\n','\n');;
            let part;
            if(colorCode === 'l'){
                bold = true;
                if(text === ' ' || text === ''){
                    return;
                }
                colorCode = prevColor;
            } 
            else if(['k', 'm', 'n', 'o'].includes(colorCode)){
                colorCode = prevColor;
            }
            if(text === ' ' || text === ''){
                prevColor = colorCode;
                return;
            }
            if(!bold){
                if(colorCode in colors){
                    part = <span style={{color:colors[colorCode]}} key={index}>{text}</span>
                }
                else{
                    part = <span key={index}>{text}</span>
                }
            } else {
                if(colorCode in colors){
                    part = <b style={{color:colors[colorCode]}} key={index}>{text}</b>
                }
                else{
                    part = <b key={index}>{text}</b>
                }
            }
            bold = false;
            this.motd.push(part);
        });
    }

    render(){
        return(
            <div className='motd'>
                {this.motd}
            </div>
        )
    }
}

export default Motd;