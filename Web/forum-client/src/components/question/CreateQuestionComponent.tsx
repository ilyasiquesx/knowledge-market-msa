import {FC, useState} from "react";
import {Box, Button, FormControl, TextareaAutosize} from "@mui/material";
import {useNavigate} from "react-router-dom";
import Typography from "@mui/material/Typography";
import * as React from "react";
import {postQuestion} from "../../services/ApiService";
import ProgressComponent from "../ProgressComponent";
import {trackPromise} from "react-promise-tracker";

interface CreateQuestionRequest {
    title: string,
    content: string,
}

const CreateQuestionComponent: FC = () => {

    const [createRequest, setQuestionRequest] = useState<CreateQuestionRequest>({
        title: '',
        content: ''
    })

    const navigate = useNavigate();

    function onFieldChange(field: string, value: any) {
        setQuestionRequest({...createRequest, [field]: value} as CreateQuestionRequest);
    }

    function onCreateQuestionHandler() {
        trackPromise(postQuestion(createRequest), 'fetch-service')
            .then((r) => {
                if (r.status === 200) {
                    navigate('/');
                }
            })
    }

    return (
        <Box sx={{
            display: 'flex',
            justifyContent: 'center',
            borderBottom: '1px solid black',
        }}>
            <FormControl sx={{
                padding: '10px',
            }}>
                <Typography variant="h4" sx={{
                    marginBottom: '10px'
                }}>Ask a new question</Typography>
                <Typography>Your title</Typography>
                <TextareaAutosize
                    style={{
                        marginTop: '10px',
                        marginBottom: '10px',
                        minHeight: '50px',
                        width: '100%'
                    }}
                    required
                    value={createRequest?.title}
                    onChange={(event) => onFieldChange("title", event.target.value)}/>
                <Typography>Your content</Typography>
                <TextareaAutosize
                    style={{
                        marginTop: '10px',
                        marginBottom: '10px',
                        minHeight: '100px',
                        width: '100%'
                    }}
                    required
                    value={createRequest?.content}
                    onChange={(event) => onFieldChange("content", event.target.value)}/>
                <Button variant="contained" onClick={onCreateQuestionHandler}>Ask</Button>
                <ProgressComponent/>
            </FormControl>
        </Box>
    )
}

export default CreateQuestionComponent;