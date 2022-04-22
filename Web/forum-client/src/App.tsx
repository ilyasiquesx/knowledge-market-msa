import React, {useState} from 'react';
import './App.css';
import ForumComponent from "./components/forum/ForumComponent";
import AccountComponent from "./components/account/AccountComponent";
import {getUser, User} from "./components/UserService";
import NavbarComponent from "./components/NavbarComponent";
import {BrowserRouter as Router, Route, Routes} from "react-router-dom";
import QuestionComponent from "./components/question/QuestionComponent";
import {Grid} from "@mui/material";
import CreateQuestionComponent from "./components/question/CreateQuestionComponent";
import {ToastContainer} from "react-toastify";
import 'react-toastify/dist/ReactToastify.css';
import UpdateQuestionComponent from "./components/question/UpdateQuestionComponent";
import NotificationsComponent from "./components/notifications/NotificationsComponent";
import UnsubscribeComponent from "./components/mailing/UnsubscribeComponent";
import EditAnswerComponent from "./components/answer/EditAnswerComponent";

function App() {

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
                            minHeight: '100vh'
                        }}>
                            <Routes>
                                <Route path="/" element={<ForumComponent/>}/>
                                <Route path="/auth" element={<AccountComponent/>}/>
                                <Route path="/question/:id" element={<QuestionComponent/>}/>
                                <Route path="/question/create" element={<CreateQuestionComponent/>}/>
                                <Route path="/question/edit/:id" element={<UpdateQuestionComponent/>}/>
                                <Route path="/notifications" element={<NotificationsComponent/>}/>
                                <Route path="/unsub/:userId" element={<UnsubscribeComponent/>}/>
                                <Route path="/answer/edit/:id/question/:questionId" element={<EditAnswerComponent/>}/>
                            </Routes>
                        </Grid>
                    </Grid>
                </Router>
                <ToastContainer
                    position="top-right"
                    autoClose={3000}
                    closeOnClick={true}
                    pauseOnHover={true}
                    draggable={true}/>
            </header>
        </div>
    );
}

export default App;
