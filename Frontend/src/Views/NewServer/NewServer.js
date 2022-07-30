import './NewServer.css';
import React from 'react';
import { ThemeContext } from '../../App';
import { BsPlusCircleFill } from "react-icons/bs";
import config from '../../config';


class NewServer extends React.Component {
    static contextType = ThemeContext;
    constructor(props){
        super(props);
        this.state = {
            name: '',
            url: '',
            premium: true,
            about: '',
            tags: [],
            currentTag: '',
            loading: false,
            message: ''
        };
        this.handleTextChange = this.handleTextChange.bind(this);
        this.handleCheckboxChange = this.handleCheckboxChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleTagSubmit = this.handleTagSubmit.bind(this);
        this.handleTagRemove = this.handleTagRemove.bind(this);  
        this.handleTagEnter = this.handleTagEnter.bind(this);
        this.hideMessage = this.hideMessage.bind(this);
    }


    handleTextChange(event){
        this.setState({
            [event.target.name]: event.target.value
        });
    }

    handleCheckboxChange(event){
        this.setState({
            [event.target.name]: event.target.checked
        });
    }

    async handleSubmit(event){
        event.preventDefault();
        if(this.state.name === '' || this.state.url === ''){
            this.setState({
                message: 'Please fill in all fields'
            });
            return;
        }
        this.setState({
            loading: true,
            message: ''
        })
        const requestOptions = {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ name: this.state.name, url: this.state.url, premium: this.state.premium, about: this.state.about, tags: this.state.tags })
        };
        const response = await fetch(`${config.url}servers`, requestOptions);
        if(response.status < 300){
            const data = await response.json();
            this.props.navigate(`/${data.serverId}`);
        } else {
            const data = await response.text();
            this.setState({
                message: data,
                loading: false
            });
        }
    }

    handleTagEnter(event){
        if(event.key === 'Enter'){
            this.handleTagSubmit(event);
        }
    }

    handleTagSubmit(event){
        if(this.state.currentTag === null || this.state.currentTag.match(/^ *$/) !== null){
            return
        }
        if(this.state.tags.includes(this.state.currentTag.trim())){
            this.setState({
                currentTag: ''
            });
            return
        }
        this.setState({
            tags: [...this.state.tags, this.state.currentTag.trim()],
            currentTag: ''
        })
        event.preventDefault();
    }

    handleTagRemove(event){
        const tagName = event.currentTarget.attributes.name.value;
        this.setState({
            tags: this.state.tags.filter(tag => tag !== tagName)
        });
    }

    hideMessage(event){
        this.setState({
            message: ''
        });
    }

    render(){
        let message = <div></div>;
        if(this.state.message !== ''){
            message = <div onClick={this.hideMessage} className={this.context === 'dark' ? 'alert alert-dark' : 'alert alert-primary'}>{this.state.message}</div>
        }
        const tags = this.state.tags.map((tag, index) => {
            return <span onClick={this.handleTagRemove} name={tag} key={tag + index.toString()} className="badge rounded-pill bg-primary m-1">{tag}</span>
        });
        return(
            <div>
                <div className='minecraft about'>
                    Submit your server
                </div>

                {message}
                <form onSubmit={e => { e.preventDefault(); }} className='row d-flex justify-content-center'>
                    <div className="mb-3 col-md-6">
                        <label className="form-label">Server name</label>
                        <input required disabled={this.state.loading} type="name" className={this.context === 'dark' ? 'darkForm form-control' : 'form-control'} name="name" value={this.state.name}  onChange={this.handleTextChange}/>
                    </div>
                    <div className="mb-3 col-md-6">
                        <label className="form-label">URL or IP</label>
                        <input required disabled={this.state.loading} type="text"  className={this.context === 'dark' ? 'darkForm form-control' : 'form-control'} name="url" value={this.state.url} onChange={this.handleTextChange}/>
                    </div>

                    <div className="mb-3 col-md-6">
                        <label className="form-label">About</label>
                        <textarea disabled={this.state.loading} rows={5} maxLength={8192} className={this.context === 'dark' ? 'darkForm form-control' : 'form-control'} name='about' value={this.state.about} onChange={this.handleTextChange} />        
                    </div>
                    <div className="mb-3 col-md-6">
                        <div className="form-check">
                            <input disabled={this.state.loading} type="checkbox" name='premium' className={this.context === 'dark' ? 'darkCheckbox form-check-input' : 'form-check-input'} checked={this.state.premium} onChange={this.handleCheckboxChange} />
                            <label className="form-check-label">Is yours server only for premium players? <code>(online-mode=true)</code></label>
                        </div>
                        <div className="form-check">
                            <div className="row g-3 align-items-center">
                                <div className='col-2'>
                                    <label className="form-label">Tags</label>
                                </div>
                                <div className="col-9">
                                    <input disabled={this.state.loading} onKeyDown={this.handleTagEnter} type="text" className={this.context === 'dark' ? 'darkForm tagInput form-control' : 'tagInput form-control'} name="currentTag" value={this.state.currentTag} onChange={this.handleTextChange}/>
                                </div>
                                <div className='col-1'>
                                    <label className="form-label" onClick={this.handleTagSubmit}><BsPlusCircleFill/></label>
                                </div>
                            </div>
                            <div className='tags'>{tags}</div>
                        </div>
                    </div>
                    <button type="button" disabled={this.state.loading} onClick={this.handleSubmit} style={{width:'50%'}} className="btn btn-primary">Submit your server!</button>
                </form>            
                </div>
        )
    }
}

export default NewServer;