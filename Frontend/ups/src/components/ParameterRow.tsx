import { useState } from "react";
import { ParameterDto } from "../api/Dtos";
import { GetParameterTypeDisplayName } from "../helpers/FormHelpers";
import { toastDefaultError, toastError } from "../helpers/ToastHelpers";
import { Api } from "../api/Api";

type ParameterRowProps = {
    parameter: ParameterDto,
    refresh: () => void,
    editParameter: (p: ParameterDto) => void,
    deleteParameter: (p: ParameterDto) => void,
    deleteOption: (optionId: number) => void,
}

export const ParameterRow = ({ parameter, refresh, editParameter, deleteParameter, deleteOption }: ParameterRowProps) => {
    const [showOptions, setShowOptions] = useState(true);
    const [addingNew, setAddingNew] = useState(false);
    const [newOptionValue, setNewOptionValue] = useState("");

    return <div>
        <div className="list-group-item">
            <div className="d-flex justify-content-between" onClick={() => {
                setShowOptions(!showOptions)
            }}>
                <label key="id" className="m-2">{parameter.id}</label>
                <label key="name" className="m-2 col-4 param-label">{parameter.name}</label>
                <label key="type" className="m-2 col-2 param-label">{GetParameterTypeDisplayName(parameter.type)}</label>
                <label key="required" className="m-2 col-1 param-label">{parameter.required ? "Wymagany" : "Opcjonalny"}</label>
                <label key="hasOptions" className="m-2 col-2 param-label-options">
                    {parameter.options.length > 0 && !showOptions && "Dostępne opcje"}
                </label>
                <button className="m-1 col-1 btn btn-sm btn-outline-primary" type="button"
                    onClick={(e) => {
                        e.stopPropagation()
                        editParameter(parameter)
                    }}
                >
                    Edytuj
                </button>
                <button className="m-1 col-1 btn btn-sm btn-outline-danger" type="button"
                    onClick={(e) => {
                        e.stopPropagation();
                        deleteParameter(parameter)
                    }}
                >
                    Usuń
                </button>
            </div>
        </div>
        {parameter.options.length > 0 && showOptions && <div className="row row-cols-5 option-container m-2">
            {parameter.options.map((o, idx) => <div key={idx} className="col option-item d-flex justify-content-between align-items-center">
                <label className="m-1">{o.value}</label>
                {parameter.options.length > 1 && <button type="button" className="btn btn-sm btn-danger mt-1 mb-1 delete-option"
                    onClick={() => {
                        Api.DeleteOption({ optionId: o.id }).then(res => {
                            if (!res.success) {
                                toastError("Nie udało się usunąć opcji " + o.value);
                                refresh();
                            }
                        })
                        deleteOption(o.id);
                    }}
                >
                    X
                </button>}
            </div>)}
            {!addingNew ?
                <div key={-1} className="col">
                    <button className="m-1 btn btn-primary" type="button" onClick={() => {
                        setAddingNew(true)
                    }}>
                        Dodaj nową opcję
                    </button>
                </div> :
                <form key={-2} className="col option-item d-flex justify-content-between align-items-center"
                    onSubmit={async (e) => {
                        e.preventDefault()
                        await Api.AddOption({ parameterId: parameter.id, value: newOptionValue }).then(res => {
                            if (res.success && res.data) {
                                refresh();
                                setAddingNew(false)
                                setNewOptionValue("")
                            }
                            else if (!!res.errors.optionId) {
                                toastError(res.errors.optionId[0])
                            }
                            else if (!!res.errors.value) {
                                toastError(res.errors.value[0])
                            }
                            else toastDefaultError();
                        })
                    }}
                >
                    <input type="text" name="value" className="form-control m-1"
                        onChange={(e) => e.target.value.length <= 256 ? setNewOptionValue(e.target.value) : undefined}
                        value={newOptionValue}
                        autoFocus
                    />
                    <button disabled={!newOptionValue}
                        type="submit" className="btn btn-sm btn-primary mt-1 mb-1 ml-1 delete-option">
                        Zapisz
                    </button>
                </form>
            }
        </div>}
    </div>
}