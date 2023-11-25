type DisplayParameterRowProps = {
    parameter: {
        id: number,
        name: string,
        answer?: string | boolean
    }
}

export const DisplayParameterRow = ({ parameter }: DisplayParameterRowProps) => {
    const answerText = parameter.answer === true ? "TAK" : parameter.answer === false ? "NIE" : parameter.answer;
    return <div>
        <div className="list-group-item">
            <div className="d-flex justify-content-between">
                <label key="id" className="m-2">{parameter.id}</label>
                <label key="name" className="m-2 col-3 param-label">{parameter.name}</label>
                <label key="answer" className="m-2 col-8 param-label">{answerText}</label>
            </div>
        </div>
    </div>
}