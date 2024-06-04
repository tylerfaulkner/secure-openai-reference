import Button from "./button"
import { useState } from "react"
import { getResponse } from "../../api/getResponse";

export default function AiPanel() {

    const [response, setResponse] = useState("Ask a question to receive a response!");
    const [value, setValue] = useState("")

    async function onClick() {
        if (!value || value === "") {
            setResponse("You must type a question in first")
            return;
        }
        setResponse("Awaiting response")
        var response = await getResponse(value);
        setResponse(response.response)
        setValue("");
    }

    return (
        <div className="w-2/5 h-1/3 bg-card-color border-2 border-border-color rounded-lg flex flex-col items-center relative">
            <div className='absolute -top-[3rem] 2xl:-top-[4rem] font-bold text-3xl 2xl:text-5xl lg:-top-[4rem] lg:text-4xl mb-10 text-center'>Azure OpenAI Test App</div>
            <div className="h-4/5 overflow-auto">
                <div className="text-lg font-medium p-4" >{response}</div>
            </div>
            <div className='flex flex-row w-full absolute bottom-0'>
                <input type='text' value={value} onChange={(e) => setValue(e.target.value)} className='flex-1 border border-border-color rounded-md p-2 w-[200px] m-5' placeholder="Ask me anything!"/>
                <Button onClick={onClick} className='bg-primary hover:bg-primary/75 m-5 rounded-md ml-0'>Ask Question</Button>
            </div>
        </div>
    )

}