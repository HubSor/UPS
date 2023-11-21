import { AnsweredParameterDto } from "../../api/Dtos"

type DisplayParameterRowProps = {
    parameter: AnsweredParameterDto
}

export const DisplayParameterRow = ({ parameter }: DisplayParameterRowProps) => {
    return <div>
        <div className="list-group-item">
            <div className="d-flex justify-content-between">
                <label key="id" className="m-2">{parameter.id}</label>
                <label key="name" className="m-2 col-3 param-label">{parameter.name}</label>
                <label key="answer" className="m-2 col-8 param-label">{parameter.answer}</label>
            </div>
        </div>
    </div>
}