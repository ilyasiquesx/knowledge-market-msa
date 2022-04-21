import * as React from 'react';
import Box from '@mui/material/Box';
import {FC} from "react";
import {LinearProgress} from "@mui/material";
import {usePromiseTracker} from "react-promise-tracker";

const ProgressComponent: FC<{}> = () => {
    const {promiseInProgress} = usePromiseTracker({area: 'fetch-service'});
    return (<Box>
        {promiseInProgress ? <Box sx={{
                width: '100%',
                marginY: '10px',
            }}>
                <LinearProgress/>
            </Box>
            : <Box/>}
    </Box>)
}

export default ProgressComponent;