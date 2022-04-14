import React from 'react';
import './App.css';
import ForumComponent from "./components/forum/ForumComponent";
import AccountComponent from "./components/account/AccountComponent";

function App() {
    return (
        <div className="App">
            <header className="App-header">
                <ForumComponent></ForumComponent>
                <AccountComponent></AccountComponent>
            </header>
        </div>
    );
}

export default App;
