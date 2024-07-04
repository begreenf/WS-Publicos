function getExchangeRate() {
    const currencyCode = document.getElementById('currency-code').value.toUpperCase();
    console.log(currencyCode)

    fetch(`/api/ExchangeRate/${currencyCode}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.text(); // Convertir respuesta a texto
        })
        .then(data => {
            let exchangeRate = parseFloat(data); // Convertir respuesta a número decimal
            document.getElementById('exchange-rate-result').innerText = `La tasa de cambio para ${currencyCode} es ${exchangeRate}`;
            document.getElementById('meaning').innerText = `${exchangeRate} DOP es igual a 1 ${currencyCode}`;
        })
        .catch(error => {
            console.error('There was a problem with the fetch operation:', error);
            document.getElementById('exchange-rate-result').innerText = `Inserte un código válido por favor`;
            document.getElementById('meaning').innerText = ``;
        });
}

async function getInflationIndex() {
    const period = document.getElementById('period').value;
    const response = await fetch(`/api/IndiceInflacion/${period}`);

    if (response.ok) {
        const data = await response.json();
        document.getElementById('inflation-index-result').innerText = `En el período ${data.periodo}, el índice de inflación fue de ${data.inflacion}%`;
    } else {
        document.getElementById('inflation-index-result').innerText = `No se encontró índice de inflación para el período ${period}`;
    }
}


async function getFinancialHealth() {
    const financialHealthRNC = document.getElementById('financialHealthID').value;

    try {
        const response = await fetch(`/api/SaludFinanciera/${financialHealthRNC}`);
        if (!response.ok) {
            throw new Error('No se encontró la salud financiera para el cliente proporcionado.');
        }

        const data = await response.json();
        document.getElementById('financial-health-result').innerText =
            `Es saludable: ${data.indicador}\nComentario: ${data.comentario}\nMonto Total: ${data.montoTotalAdeudado}`;
    } catch (error) {
        document.getElementById('financial-health-result').innerText = error.message;
    }
}



function getCreditHistory() {
    const creditHistoryRNC = document.getElementById('creditHistoryID').value;

    fetch(`/api/HistorialCrediticio/${creditHistoryRNC}`)
        .then(response => response.json())
        .then(data => {
            let resultText = '';
            if (data.length > 0) {
                data.forEach(item => {
                    resultText += `RNC: ${item.companyRNC}\nConcepto Deuda: ${item.conceptoDeuda}\nFecha Deuda: ${item.fechaDeuda}\nMonto Total: ${item.montoTotalAdeudado}\n\n`;
                });
            } else {
                resultText = 'No se encontró historial crediticio.';
            }
            document.getElementById('credit-history-result').innerText = resultText;
        })
        .catch(error => {
            console.error('Error:', error);
            document.getElementById('credit-history-result').innerText = 'Ocurrió un error al consultar el historial crediticio.';
        });
}