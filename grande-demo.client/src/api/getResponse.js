
export async function getResponse(request) {
    let body = {
        request: request
    };
    
    let response = await fetch("api/AI/GetResponse", 
    {
        method: "POST",
        headers: {
            "Content-Type": "application/json; charset=utf-8",
        },
        body: JSON.stringify(body)
    });
    return await response.json();
}