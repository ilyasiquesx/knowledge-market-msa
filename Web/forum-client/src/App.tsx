import React, {useEffect, useState} from 'react';
import './App.css';
import ForumComponent from "./components/forum/ForumComponent";
import AccountComponent from "./components/account/AccountComponent";
import {getUser, isAuthenticated, User} from "./components/UserService";
import NavbarComponent from "./components/NavbarComponent";
import {BrowserRouter as Router, Route, Routes} from "react-router-dom";
import QuestionComponent from "./components/forum/QuestionComponent";

function App() {

    const [user, setUser] = useState<User>(getUser());

    console.log(user);
    return (
        <div className="App">
            <header className="App-header">
                <Router>
                    <NavbarComponent/>
                    <Routes>
                        <Route path="/" element={<ForumComponent/>}/>
                        <Route path="/auth" element={<AccountComponent/>}/>
                        <Route path="/question/:id" element={<QuestionComponent/>}/>
                    </Routes>
                </Router>
            </header>
        </div>
    );
}

export default App;
