import { SalePathStepProps } from "../../pages/SalePathPage"

type ChooseSubProductsProps = SalePathStepProps

export const ChooseSubProductsStep = ({ state, dispatch }: ChooseSubProductsProps) => {
    return <div>
        <h3>Wybierz podprodukty</h3>
        {state.subProductIds.length === 0 && state.product && state.product.subProducts.length > 0 && <div className="alert alert-warning">
            Nie wybrano żadnego podproduktu.
        </div>}
        <br/>
        {!!state.product && <table className="table table-striped col-lg-3">
            <thead>
                <tr className="table-dark">
                    <th scope="col">Kod</th>
                    <th scope="col">Nazwa</th>
                    <th scope="col">Cena</th>
                </tr>
            </thead>
            <tbody>
                {state.product.subProducts.map(sp => {
                    return <tr key={sp.id} onClick={() => {
                        if (!state.subProductIds.includes(sp.id))
                            dispatch({ type: 'addSubProduct', subProductId: sp.id })
                        else
                            dispatch({ type: 'removeSubProduct', subProductId: sp.id })
                    }} className={state.subProductIds.includes(sp.id) ? "table-primary" : ""}>
                        <td>
                            {sp.code}
                        </td>
                        <td>
                            {sp.name}
                        </td>
                        <td>
                            {sp.price}
                        </td>
                    </tr>
                })}
            </tbody>
        </table>}
    </div>
}