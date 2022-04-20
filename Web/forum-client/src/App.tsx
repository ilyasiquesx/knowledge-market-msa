import React, {useEffect, useState} from 'react';
import './App.css';
import ForumComponent from "./components/forum/ForumComponent";
import AccountComponent from "./components/account/AccountComponent";
import {getUser, isAuthenticated, User} from "./components/UserService";
import NavbarComponent from "./components/NavbarComponent";
import {BrowserRouter as Router, Route, Routes} from "react-router-dom";
import QuestionComponent from "./components/forum/QuestionComponent";
import {Grid} from "@mui/material";
import CreateQuestionComponent from "./components/forum/CreateQuestionComponent";
import {ToastContainer} from "react-toastify";
import 'react-toastify/dist/ReactToastify.css';
import UpdateQuestionComponent from "./components/forum/UpdateQuestionComponent";
import Box from "@mui/material/Box";

function App() {

    const [user, setUser] = useState<User>(getUser());

    return (
        <div className="App">
            <header className="App-header">
                <Router>
                    <NavbarComponent/>
                    <Grid
                        container
                        spacing={0}
                        direction="row"
                        alignItems="center"
                        justifyContent="center"
                        width="100%"
                        margin="auto"
                    >
                        <Grid item xl={6} md={8} xs={12} sx={{
                            borderRight: '1px solid #7194a8',
                            borderLeft: '1px solid #7194a8',
                            height: '100vh'
                        }}>
                            <Routes>
                                <Route path="/" element={<ForumComponent/>}/>
                                <Route path="/auth" element={<AccountComponent/>}/>
                                <Route path="/question/:id" element={<QuestionComponent/>}/>
                                <Route path="/question/create" element={<CreateQuestionComponent/>}/>
                                <Route path="/question/edit/:id" element={<UpdateQuestionComponent/>}/>
                            </Routes>
                        </Grid>
                    </Grid>
                </Router>
                <ToastContainer/>
            </header>
        </div>
    );
}

export default App;
